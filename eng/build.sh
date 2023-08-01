#!/usr/bin/env bash

set -ue

source="${BASH_SOURCE[0]}"

# resolve $source until the file is no longer a symlink
while [[ -h "$source" ]]; do
  scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
  source="$(readlink "$source")"
  # if $source was a relative symlink, we need to resolve it relative to the path where the
  # symlink file was located
  [[ $source != /* ]] && source="$scriptroot/$source"
done
scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"

usage()
{
  echo "Common settings:"
  echo "  --configuration <value>  Build configuration: Debug or Release (short: -c)"
  echo "  --verbosity <value>      MSBuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic] (short: -v)"
  echo "  --binaryLog              Output binary log (short: -bl)"
  echo "  --help                   Print help and exit (short: -h)"
  echo ""

  echo "Actions (defaults to --restore --build):"
  echo "  --restore                  Restore dependencies (short: -r)"
  echo "  --build                    Build all source projects (short: -b)"
  echo "  --rebuild                  Rebuild all source projects"
  echo "  --test                     Run all unit tests (short: -t)"
  echo "  --pack                     Package build outputs into NuGet packages"
  echo "  --sign                     Sign build outputs"
  echo "  --publish                  Publish artifacts (e.g. symbols)"
  echo "  --clean                    Clean the solution"
  echo ""
}

arguments=''
extraargs=''

# Check if an action is passed in
declare -a actions=("r" "restore" "b" "build" "rebuild" "t" "test" "pack" "sign" "publish" "clean")
actInt=($(comm -12 <(printf '%s\n' "${actions[@]/#/-}" | sort) <(printf '%s\n' "${@/#--/-}" | sort)))

while [[ $# > 0 ]]; do
  opt="$(echo "${1/#--/-}" | tr "[:upper:]" "[:lower:]")"

  case "$opt" in
     -help|-h|-\?|/?)
      usage
      exit 0
      ;;

     -configuration|-c)
      if [ -z ${2+x} ]; then
        echo "No configuration supplied. See help (--help) for supported configurations." 1>&2
        exit 1
      fi
      passedConfig="$(echo "$2" | tr "[:upper:]" "[:lower:]")"
      case "$passedConfig" in
        debug|release)
          val="$(tr '[:lower:]' '[:upper:]' <<< ${passedConfig:0:1})${passedConfig:1}"
          ;;
        *)
          echo "Unsupported configuration '$2'."
          echo "The allowed values are Debug or Release."
          exit 1
          ;;
      esac
      arguments="$arguments -configuration $val"
      shift 2
      ;;

      *)
      extraargs="$extraargs $1"
      shift 1
      ;;
  esac
done

if [ ${#actInt[@]} -eq 0 ]; then
    arguments="-restore -build $arguments"
fi

arguments="$arguments $extraargs"
"$scriptroot/common/build.sh" $arguments
