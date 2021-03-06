DROP TABLE IF EXISTS `AuthClients`;
CREATE TABLE IF NOT EXISTS `AuthClients` (
	`Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	`ClientId` VARCHAR(64) NOT NULL,
	`Secret` VARCHAR(64) NOT NULL,
	`Name` VARCHAR(64) NOT NULL
);

DROP TABLE IF EXISTS `AuthRequests`;
CREATE TABLE IF NOT EXISTS `AuthRequests` (
	Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	Guid VARCHAR(36) NOT NULL,
	ClientId VARCHAR(64) NOT NULL,
	RequestedScope VARCHAR(512),
	RedirectUri VARCHAR(64) NOT NULL,
	Platform VARCHAR(20),
	Version VARCHAR(20),
	Idiom VARCHAR(20),
	Code VARCHAR(1024),
	Error VARCHAR(512),
	AccessToken VARCHAR(512),
	TokenType VARCHAR(20),
	AuthorizedScope VARCHAR(512),
	ExpiresIn INT,
	RefreshToken VARCHAR(512),
	RequestTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	CodeTime TIMESTAMP,
	TokenTime TIMESTAMP,
	INDEX Guid (Guid),
	INDEX ClientGuid (ClientId, Guid)
);
