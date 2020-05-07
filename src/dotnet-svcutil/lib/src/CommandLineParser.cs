// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal enum SwitchType
    {
        Flag,
        SingletonValue,
        ValueList,
        FlagOrSingletonValue
    }

    internal class CommandSwitch
    {
        public const string AbbreviationSwitchIndicator = "-";
        public const string FullSwitchIndicator = "--";

        public const OperationalContext DefaultSwitchLevel = OperationalContext.Project;

        private static List<CommandSwitch> s_allSwitches = new List<CommandSwitch>();
        public static IEnumerable<CommandSwitch> All { get { return s_allSwitches; } }
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }
        public SwitchType SwitchType { get; private set; }
        public OperationalContext SwitchLevel { get; private set; }

        internal CommandSwitch(string name, string abbreviation, SwitchType switchType, OperationalContext switchLevel = DefaultSwitchLevel)
        {
            // ensure that name doesn't start with the switch indicators.
            // also convert to lower-case
            if (name.StartsWith(FullSwitchIndicator))
            {
                this.Name = name.Substring(FullSwitchIndicator.Length);
            }
            else
            {
                this.Name = name;
            }

            if (abbreviation.StartsWith(AbbreviationSwitchIndicator))
            {
                this.Abbreviation = abbreviation.Substring(AbbreviationSwitchIndicator.Length);
            }
            else
            {
                this.Abbreviation = abbreviation;
            }

            this.SwitchType = switchType;
            this.SwitchLevel = switchLevel;

            System.Diagnostics.Debug.Assert(!s_allSwitches.Any(s => s.Equals(this)), $"A switch with name or abbreviation '{name}+{abbreviation}' has already been crated!");

            s_allSwitches.Add(this);
        }

        internal static CommandSwitch FindSwitch(string name)
        {
            return CommandSwitch.All.FirstOrDefault(s => s.Equals(name));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {{{1}, {2}, Type={3}}}", this.GetType().Name, Name, Abbreviation, SwitchType);
        }

        internal bool Equals(string other)
        {
            string otherName;
            bool isAbbreviation = true;

            if (string.IsNullOrWhiteSpace(other))
            {
                return false;
            }

            // If other starts with a switch indicator, we want to enforce the correct indicator (e.g. "-directory" and "--d" will return false, but "--directory" and "directory" will return true).
            bool enforceSwitchType = other.StartsWith(FullSwitchIndicator) || other.StartsWith(AbbreviationSwitchIndicator);

            // Take off the switch indicators and figure out if this is an abbreviation or not.
            // also convert to lower-case
            if (other.StartsWith(FullSwitchIndicator))
            {
                isAbbreviation = false;
                otherName = other.Substring(FullSwitchIndicator.Length).ToLowerInvariant();
            }
            else if (other.StartsWith(AbbreviationSwitchIndicator))
            {
                isAbbreviation = true;
                otherName = other.Substring(AbbreviationSwitchIndicator.Length).ToLowerInvariant();
            }
            else
            {
                otherName = other.ToLowerInvariant();
            }

            var thisAbbr = Abbreviation.ToLowerInvariant();
            var thisName = Name.ToLowerInvariant();

            if (enforceSwitchType)
            {
                return isAbbreviation ? thisAbbr == otherName : thisName == otherName;
            }
            else
            {
                return thisAbbr == otherName || thisName == otherName;
            }
        }
    }

    internal static class CommandParser
    {
        // Instead of throwing exceptions we pass the first exception that happens as an out parameter. This lets us parse as many
        // arguments as we can correctly so we can respect options like noLogo and verbosity when showing errors.
        internal static CommandProcessorOptions ParseCommand(string[] cmd)
        {
            var options = new CommandProcessorOptions();

            // work around https://github.com/dotnet/core-setup/issues/619 to ignore additionalprobingpath
            cmd = cmd.Where(c => !c.ToLowerInvariant().Contains("additionalprobingpath")).ToArray();

            for (int i = 0; i < cmd.Length; i++)
            {
                string arg = cmd[i]?.Trim('\"').Trim();

                if (string.IsNullOrWhiteSpace(arg))
                {
                    continue;
                }

                // if argument does not start with switch indicator, place into "default" arguments
                if (IsArgumentValue(arg))
                {
                    SetValue(options, CommandProcessorOptions.InputsKey, Environment.ExpandEnvironmentVariables(arg));
                    continue;
                }

                // check if this switch exists in the list of possible switches
                var argSwitch = CommandSwitch.FindSwitch(arg);
                var argValue = cmd.Length > i + 1 ? cmd[i + 1] : null;

                if (argSwitch == null)
                {
                    options.AddError(string.Format(SR.ErrUnknownSwitchFormat, arg));
                    continue;
                }

                // we have a valid switch, now validate value.
                // make sure there's a value for the parameter.
                switch (argSwitch.SwitchType)
                {
                    case SwitchType.Flag:
                        argValue = "true";
                        break;
                    case SwitchType.FlagOrSingletonValue:
                        argValue = (argValue == null || !IsArgumentValue(argValue)) ? string.Empty : argValue;
                        break;
                    case SwitchType.SingletonValue:
                    case SwitchType.ValueList:
                        if (string.IsNullOrWhiteSpace(argValue) || !IsArgumentValue(argValue))
                        {
                            options.AddError(string.Format(SR.ErrArgumentWithoutValue, argSwitch.Name));
                            continue;
                        }
                        break;
                }

                // check if switch is allowed to be specified multiple times
                // if not and it has already been specified and a new value has been parsed.
                if (argSwitch.SwitchType != SwitchType.ValueList && options.GetValue<object>(argSwitch.Name) != null)
                {
                    options.AddError(string.Format(SR.ErrSingleUseSwitchFormat, argSwitch.Name));
                    continue;
                }

                SetValue(options, argSwitch.Name, argValue.Trim('\"').Trim());
                if (argSwitch.SwitchType != SwitchType.Flag && argValue != string.Empty && IsArgumentValue(argValue))
                {
                    i++; // move to next input.
                }
            }

            return options;
        }

        private static void SetValue(CommandProcessorOptions options, string optionName, object value)
        {
            try
            {
                options.SetValue(optionName, value);
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalOrUnexpected(ex)) throw;
                options.AddError(ex);
            }
        }

        private static bool IsArgumentValue(string arg)
        {
            return !(arg.StartsWith(CommandSwitch.FullSwitchIndicator) || arg.StartsWith(CommandSwitch.AbbreviationSwitchIndicator));
        }
    }
}
