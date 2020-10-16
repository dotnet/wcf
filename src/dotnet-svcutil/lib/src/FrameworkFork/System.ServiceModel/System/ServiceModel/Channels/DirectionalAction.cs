// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Collections.Generic;
    internal class DirectionalAction : IComparable<DirectionalAction>
    {
        private MessageDirection _direction;
        private string _action;
        private bool _isNullAction;

        internal DirectionalAction(MessageDirection direction, string action)
        {
            if (!MessageDirectionHelper.IsDefined(direction))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("direction"));

            _direction = direction;
            if (action == null)
            {
                _action = MessageHeaders.WildcardAction;
                _isNullAction = true;
            }
            else
            {
                _action = action;
                _isNullAction = false;
            }
        }

        public MessageDirection Direction
        { get { return _direction; } }

        public string Action
        { get { return _isNullAction ? null : _action; } }

        public override bool Equals(Object other)
        {
            DirectionalAction tmp = other as DirectionalAction;
            if (tmp == null)
                return false;
            return this.Equals(tmp);
        }

        public bool Equals(DirectionalAction other)
        {
            if (other == null)
                return false;

            return (_direction == other._direction)
                && (_action == other._action);
        }

        public int CompareTo(DirectionalAction other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");

            if ((_direction == MessageDirection.Input) && (other._direction == MessageDirection.Output))
                return -1;
            if ((_direction == MessageDirection.Output) && (other._direction == MessageDirection.Input))
                return 1;

            return _action.CompareTo(other._action);
        }

        public override int GetHashCode()
        {
            return _action.GetHashCode();
        }
    }
}
