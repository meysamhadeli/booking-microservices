#!/bin/bash

# update package version and version for in the csproj file to new version and commit it again with using https://github.com/semantic-release/git plugin
# https://unix.stackexchange.com/questions/50313/how-do-i-perform-an-action-on-all-files-with-a-specific-extension-in-subfolders
find . -name '*.Packages.props' -exec sed -i "s#<PackageVersion>.*#<PackageVersion>$1</PackageVersion>#" {} \;  -exec cat {} \;
find . -name '*.Packages.props' -exec sed -i "s#<InformationalVersion>.*#<InformationalVersion>$1</InformationalVersion>#" {} \;  -exec cat {} \;
find . -name '*.Packages.props' -exec sed -i "s#<Version>.*#<Version>$1</Version>#" {} \;  -exec cat {} \;
