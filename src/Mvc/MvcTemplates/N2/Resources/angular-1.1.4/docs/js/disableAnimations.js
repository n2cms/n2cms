'use strict';

console.log('#1 disableAnimations.js loaded');
angular.module('disableAnimations', []).config(function($provide) {
  console.log('#2 disableAnimations module loaded');
  $provide.decorator('$sniffer', function($delegate) {
    console.log('#3 sniffer decorated');
    $delegate.supportsTransitions = false;
    return $delegate;
  });
});
