#!/bin/bash -ex

set -o pipefail

for i in `seq 19`; do
    if  curl http://bus:4566 -H 'content-type: application/json' | egrep running ; then 
        break 
    else 
        sleep 19
    fi
done

dotnet test --filter=Type=Integration