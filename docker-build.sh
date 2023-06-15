#!/bin/bash -e

docker-compose build --no-cache --build-arg DOCKER_IMAGE_TAG=$DOCKER_IMAGE_TAG
