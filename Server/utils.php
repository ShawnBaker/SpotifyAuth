<?php
//************************************************************************
// clean_request_input
//************************************************************************
function clean_request_input($data)
{
    $data = trim($data);
    $data = stripslashes($data);
    $data = htmlspecialchars($data);
    return $data;
}

//************************************************************************
// page_begin
//************************************************************************
function page_begin($title)
{
	echo "<!DOCTYPE HTML PUBLIC  \"-//W3C//DTD HTML 4.0//EN\">\n";
	echo "<html>\n\n";
	echo "<head>\n";
	echo "<meta http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">\n";
	echo "<link rel=\"stylesheet\" type=\"text/css\" href=\"default.css\">\n";
	echo "<title>" . $title . "</title>\n";
	echo "</head>\n<body>\n\n";
}

//************************************************************************
// page_end
//************************************************************************
function page_end()
{
	echo "\n</body>\n</html>\n";
}

//************************************************************************
// error_page
//************************************************************************
function error_page($error)
{
    page_begin("ERROR");
	echo "<div class=\"center\">$error</div>\n";
    page_end();
}

//************************************************************************
// mylog
//************************************************************************
function mylog($name, $value)
{
	file_put_contents("request.log", $name . ": " . print_r($value, true) . PHP_EOL, FILE_APPEND);
}
?>
