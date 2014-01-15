#!/bin/bash

expand_func () {
  dos2unix -m -n "$1" "$1.tmp"
  mv "$1.tmp" "$1"
}

export -f expand_func

find . -name \*.aspx -exec bash -c 'expand_func {}' \;

