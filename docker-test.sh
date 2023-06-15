#!/bin/bash -e

echo "Run Unit Tests"
docker compose -f docker-compose-unit.yml up --abort-on-container-exit --exit-code-from tests

echo "Run Integration Tests"
docker compose -f docker-compose-integration.yml up --abort-on-container-exit --exit-code-from tests
