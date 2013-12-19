#!/bin/bash

expand_func () {
  expand -t 4 "$1" > "$1.tmp"
  mv "$1.tmp" "$1"
}

export -f expand_func

find src -name \*.cs -exec bash -c 'expand_func {}' \;

