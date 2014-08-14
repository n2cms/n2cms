<?php

require_once('gen.config.php');

function calculateTextBox($font_size, $font_angle, $font_file, $text) {
	$box   = imagettfbbox($font_size, $font_angle, $font_file, $text);
	if( !$box )
		return false;
	$min_x = min( array($box[0], $box[2], $box[4], $box[6]) );
	$max_x = max( array($box[0], $box[2], $box[4], $box[6]) );
	$min_y = min( array($box[1], $box[3], $box[5], $box[7]) );
	$max_y = max( array($box[1], $box[3], $box[5], $box[7]) );
	$width  = ( $max_x - $min_x );
	$height = ( $max_y - $min_y );
	$left   = abs( $min_x ) + $width;
	$top    = abs( $min_y ) + $height;
	// to calculate the exact bounding box i write the text in a large image
	$img     = @imagecreatetruecolor( $width << 2, $height << 2 );
	$white   =  imagecolorallocate( $img, 255, 255, 255 );
	$black   =  imagecolorallocate( $img, 0, 0, 0 );
	imagefilledrectangle($img, 0, 0, imagesx($img), imagesy($img), $black);
	// for sure the text is completely in the image!
	imagettftext( $img, $font_size,
	$font_angle, $left, $top,
	$white, $font_file, $text);
	// start scanning (0=> black => empty)
	$rleft  = $w4 = $width<<2;
	$rright = 0;
	$rbottom   = 0;
	$rtop = $h4 = $height<<2;
	for( $x = 0; $x < $w4; $x++ )
		for( $y = 0; $y < $h4; $y++ )
		if( imagecolorat( $img, $x, $y ) ){
		$rleft   = min( $rleft, $x );
		$rright  = max( $rright, $x );
		$rtop    = min( $rtop, $y );
		$rbottom = max( $rbottom, $y );
	}
	// destroy img and serve the result
	imagedestroy( $img );

	return array( "left"   => $left - $rleft,
			"top"    => $top  - $rtop,
			"width"  => $rright - $rleft + 1,
			"height" => $rbottom - $rtop + 1 );
}


if (!isset($_GET['code']))
	die("`code` parameter not set");

if (!isset($_GET['size']))
	die("`size` parameter not set");

if (!isset($_GET['color']))
	die("`color` parameter not set");
$colorHtml = strtolower($_GET['color']);
if (!preg_match('/^[0-9a-f]{6}$/', $colorHtml) == 1)
    die('Incorrect color code');

$colorR = base_convert(substr($colorHtml, 0, 2), 16, 10);
$colorG = base_convert(substr($colorHtml, 2, 2), 16, 10);
$colorB = base_convert(substr($colorHtml, 4, 2), 16, 10);

$code = intval($_GET['code']);
$size = intval($_GET['size']);

if ($code < 0 || $code > 65535)
	die('Incorrect `code` value');

if ($size < $config['minSize'])
    $size = $config['minSize'];

if ($size > $config['maxSize'])
    $size = $config['maxSize'];

$fontFile = dirname(__FILE__).'/icons.ttf';
$text = html_entity_decode('&#' . $code . ';', ENT_COMPAT, 'UTF-8');
//$dim = imagettfbbox($size, 0,  $fontFile, $text);

$dim = calculateTextBox($size, 0, $fontFile, $text);

/*$dim = array(
    'width': $size,
    'height' : $size,
    'left' : 0,
    'top': 0
);*/

$width = $dim['width'];
$height = $dim['height'];
$left = $dim['left'];
$top = $dim['top'];

$img = @imagecreatetruecolor($width, $height) or die("Cannot Initialize new GD image stream. Check GD library in installed on server and selected font for existence.");
imagesavealpha($img, true);
$trans_colour = imagecolorallocatealpha($img, 0, 0, 0, 127);
imagefill($img, 0, 0, $trans_colour);

$color = imagecolorallocate($img, $colorR, $colorG, $colorB);

imagettftext($img, $size, 0, $left, $top, $color, $fontFile, $text);

header("Content-type: image/png");
imagepng($img);
imagedestroy($img);

?>