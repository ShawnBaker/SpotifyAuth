<?php
include_once 'utils.php';
include_once 'database.php';

//mylog("redirect", $_REQUEST);

// get the request values
$code = clean_request_input($_REQUEST["code"]);
$guid = clean_request_input($_REQUEST["state"]);
$error = clean_request_input($_REQUEST["error"]);

// open the database connection
if (empty($guid))
{
	error_page("Invalid code response.");
	return false;
}
$db = new Database();
	
// get the request
$request = $db->get_request($guid);
if (empty($request))
{
	error_page("Invalid code guid.");
	$db->close();
	return false;
}

// update the database
$db->add_code($guid, $code, $error);
$db->close();
?>