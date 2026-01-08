#!/bin/bash

root_dir=$(git rev-parse --show-toplevel)

cd $root_dir

dotnet new install ./libraries/_Template
dotnet new install ./libraries/_PackageTemplate
