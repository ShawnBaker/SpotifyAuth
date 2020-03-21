<?php
class Database
{
	private $host		= 'host';
	private $user		= 'username';
	private $password	= 'password';
	private $database	= 'database';
	private $conn		= null;

	//************************************************************************
	//	__construct
	//************************************************************************
	function __construct()
	{
		$this->conn = new mysqli($this->host, $this->user, $this->password, $this->database);
		if ($this->conn->connect_error)
		{
			return false;
		}
	}
	
	//************************************************************************
	//	close
	//************************************************************************
	function close()
	{
		$this->conn->close();
	}
	
	//************************************************************************
	//	get_client
	//************************************************************************
	function get_client($client_id)
	{
		// make sure we're connected to the database
		if ($this->conn->connect_error)
		{
			return null;
		}
		
		// perform the query
		$sql = sprintf("SELECT * FROM AuthClients WHERE ClientId='%s'", $client_id);
		return $this->query($sql);
	}

	//************************************************************************
	//	get_request
	//************************************************************************
	function get_request($guid)
	{
		// make sure we're connected to the database
		if ($this->conn->connect_error)
		{
			return null;
		}
		
		// perform the query
		$sql = sprintf("SELECT * FROM AuthRequests WHERE Guid='%s'", $guid);
		return $this->query($sql);
	}

	//************************************************************************
	//	add_request
	//************************************************************************
	function add_request($guid, $client_id, $scope, $redirect_uri, $platform, $version, $idiom)
	{
		// make sure we're connected to the database
		if ($this->conn->connect_error)
		{
			return null;
		}
		
		// perform the query
		$sql = sprintf("INSERT INTO AuthRequests(Guid,ClientId,RequestedScope,RedirectUri,Platform,Version,Idiom) " .
						"VALUES('%s','%s','%s','%s','%s','%s','%s');", $guid, $client_id, $scope, $redirect_uri,
						$platform, $version, $idiom);
		$q = $this->conn->query($sql);
		return !empty($q) && $q == 1;
	}
	
	//************************************************************************
	//	add_code
	//************************************************************************
	function add_code($guid, $code, $error)
	{
		// make sure we're connected to the database
		if ($this->conn->connect_error)
		{
			return null;
		}
		
		// perform the query
		$sql = sprintf("UPDATE AuthRequests SET Code='%s',Error='%s',CodeTime=NOW() WHERE Guid='%s';", $code, $error, $guid);
		$q = $this->conn->query($sql);
		return !empty($q) && $q == 1;
	}
	
	//************************************************************************
	//	add_token
	//************************************************************************
	function add_token($guid, $access_token, $token_type, $scope, $expires_in, $refresh_token)
	{
		// make sure we're connected to the database
		if ($this->conn->connect_error)
		{
			return null;
		}
		
		// perform the query
		$sql = sprintf("UPDATE AuthRequests SET AccessToken='%s',TokenType='%s'," .
						"AuthorizedScope='%s',ExpiresIn=%s,RefreshToken='%s',TokenTime=NOW() WHERE Guid='%s';",
						$access_token, $token_type, $scope, $expires_in, $refresh_token, $guid);
		$q = $this->conn->query($sql);
		return !empty($q) && $q == 1;
	}
	
	//************************************************************************
	//	query
	//************************************************************************
	function query($sql)
	{
		// perform the query
		$q = $this->conn->query($sql);
		if ($q->num_rows != 1)
		{
			return null;
		}

		// get the row
		$row = $q->fetch_assoc();
		if (!$row)
		{
			return null;
		}

		// return the row
		return $row;
	}
}
?>
