#!/bin/bash

root_dir=$(git rev-parse --show-toplevel)

cd "$root_dir"

mkdir -p .githooks
git config core.hooksPath .githooks

bash ./scripts/install_templates.sh
bash ./scripts/build_local_nuget.sh
