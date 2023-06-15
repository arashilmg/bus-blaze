#!/bin/bash -e

docker-compose -f docker-compose-unit.yml up --abort-on-container-exit --exit-code-from tests
docker-compose -f docker-compose-integration.yml up --abort-on-container-exit --exit-code-from tests
docker-compose -f docker-compose-component.yml up --abort-on-container-exit --exit-code-from tests
