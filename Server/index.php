<?php
include_once 'utils.php';
include_once 'database.php';

//************************************************************************
// get_token
//************************************************************************
function get_token($guid, $auth_code)
{
	// open the database connection
	if (empty($guid) or empty($auth_code))
	{
		error_page("Invalid token request.");
		return false;
	}
	$db = new Database();
	
	// get the client id, secret and redirect URI
	$request = $db->get_request($guid);
	if (empty($request))
	{
		error_page("Invalid token guid.");
		$db->close();
		return false;
	}
	$client_id = $request["ClientId"];
	$client = $db->get_client($client_id);
	if ($client == null)
	{
		error_page("Invalid token client.");
		$db->close();
		return false;
	}
	$secret = $client["Secret"];
	$redirect_uri = $request["RedirectUri"];

	// request the tokens
    $curl = curl_init("https://accounts.spotify.com/api/token");
    curl_setopt($curl, CURLOPT_HTTPHEADER, ["Authorization: Basic " . base64_encode($client_id . ":" . $secret)]);
    curl_setopt($curl, CURLOPT_POST, true);
    curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($curl, CURLOPT_POSTFIELDS, http_build_query([
		"grant_type" => "authorization_code",
		"code" => $auth_code,
		"redirect_uri" => $redirect_uri
    ]));
    $json = curl_exec($curl);
    if (!$json)
    {
        trigger_error(curl_error($curl));
    }
    curl_close($curl);
	
	// update the database
	$obj = json_decode($json);
	$access_token = $obj->{"access_token"};
	$token_type = $obj->{"token_type"};
	$scope = $obj->{"scope"};
	$expires_in = $obj->{"expires_in"};
	$refresh_token = $obj->{"refresh_token"};
	$db->add_token($guid, $access_token, $token_type, $scope, $expires_in, $refresh_token);
	$db->close();
	
	// return the json
    return $json;
}

//************************************************************************
// refresh_token
//************************************************************************
function refresh_token($guid, $refresh_token)
{
	// open the database connection
	if (empty($guid) or empty($refresh_token))
	{
		error_page("Invalid refresh request.");
		return false;
	}
	$db = new Database();
	$db->close();
	
	// get the client id, secret and refresh token
	$request = $db->get_request($guid);
	if (empty($request))
	{
		error_page("Invalid token guid.");
		$db->close();
		return false;
	}
	$client_id = $request["ClientId"];
	$client = $db->get_client($client_id);
	if ($client == null)
	{
		error_page("Invalid token client.");
		$db->close();
		return false;
	}
	$secret = $client["Secret"];
	$refresh_token = $request["RefreshToken"];

	// request the token
    $curl = curl_init("https://accounts.spotify.com/api/token");
    curl_setopt($curl, CURLOPT_HTTPHEADER, ["Authorization: Basic " . base64_encode($client_id . ":" . $secret)]);
    curl_setopt($curl, CURLOPT_POST, true);
    curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($curl, CURLOPT_POSTFIELDS, http_build_query([
		"grant_type" => "refresh_token",
		"refresh_token" => $refresh_token
    ]));
    $result = curl_exec($curl);
    if (!$result)
    {
        trigger_error(curl_error($curl));
    }
    curl_close($curl);
	/*
	// update the database
	$obj = json_decode($json);
	$access_token = $obj->{"access_token"};
	$token_type = $obj->{"token_type"};
	$scope = $obj->{"scope"};
	$expires_in = $obj->{"expires_in"};
	$refresh_token = $obj->{"refresh_token"};
	$db->add_token($guid, $access_token, $token_type, $scope, $expires_in, $refresh_token);
	$db->close();
	*/
	// return the json
    return $result;
}

//************************************************************************
// get_auth_code
//************************************************************************
function get_auth_code($guid, $client_id, $scope, $redirect_uri, $platform, $version, $idiom)
{
	// open the database connection
	if (empty($guid) or empty($client_id) or empty($scope) or empty($redirect_uri) or
		empty($platform) or empty($version) or empty($idiom))
	{
		error_page("Invalid auth request.");
		return false;
	}
	$db = new Database();
	
	// get the client record
	$client = $db->get_client($client_id);
	if ($client == null)
	{
		error_page("Invalid auth client.");
		$db->close();
		return false;
	}
	
	// add a request record
	$request = $db->get_request($guid);
	if (!empty($request))
	{
		error_page("Duplicate auth request.");
		$db->close();
		return false;
	}
	$ok = $db->add_request($guid, $client_id, $scope, $redirect_uri, $platform, $version, $idiom);
	$db->close();
	
	// redirect to the authorization page
	$location = "https://accounts.spotify.com/en/authorize/?" . http_build_query(
	[
		"response_type" => "code",
		"client_id" => $client_id,
		"scope" => $scope,
		"redirect_uri" => $redirect_uri,
		"state" => $guid,
		"show_dialog" => 'false'
	]);
	header("Location:" . $location, true, 303);
	exit();
}

//************************************************************************
// mainline
//************************************************************************
//mylog("index", $_REQUEST);

// perform the requested action
$action = clean_request_input($_REQUEST["action"]);
$guid = clean_request_input($_REQUEST["guid"]);
if ($action === "token")
{
    $code = clean_request_input($_POST["value"]);
    echo get_token($guid, $code);
}
else if ($action === "refresh")
{
    $refresh_token = clean_request_input($_POST["value"]);
    echo refresh_token($guid, $refresh_token);
}
else if ($action === "auth")
{
	$client_id = clean_request_input($_GET["client_id"]);
    $scope = clean_request_input($_GET["scope"]);
	$redirect_uri = clean_request_input($_GET["redirect_uri"]);
    $platform = clean_request_input($_GET["platform"]);
    $version = clean_request_input($_GET["version"]);
    $idiom = clean_request_input($_GET["idiom"]);
    /*
    page_begin("grant_type=".$grant_type);
    echo "<p>$action</p>";
    echo "<p>$guid</p>";
    echo "<p>$client_id</p>";
    echo "<p>$scope</p>";
    echo "<p>$redirect_uri</p>";
    */
    get_auth_code($guid, $client_id, $scope, $redirect_uri, $platform, $version, $idiom);
    //page_end();
}
else
{
    error_page("Unknown request.");
}
?>