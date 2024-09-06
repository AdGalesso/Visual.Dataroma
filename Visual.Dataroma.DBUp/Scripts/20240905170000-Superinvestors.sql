CREATE TABLE Superinvestors (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY, -- GUID as primary key
    PortfolioManager NVARCHAR(255) NOT NULL, -- Name of the Portfolio Manager
    PortfolioValue DECIMAL(18, 2) NOT NULL, -- Decimal for portfolio value
    NumberOfStocks INT NOT NULL, -- Integer for the number of stocks
    ManagerLink NVARCHAR(255) NOT NULL, -- Link as NVARCHAR
    ManagerBase64 TEXT NULL,
    UpdatedAt DATETIME NOT NULL -- Date and time of the update
);