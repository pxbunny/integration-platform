#!/bin/bash

root_dir=$(git rev-parse --show-toplevel)

cd $root_dir

dotnet new uninstall ./libraries/_Template
dotnet new uninstall ./libraries/_PackageTemplate
