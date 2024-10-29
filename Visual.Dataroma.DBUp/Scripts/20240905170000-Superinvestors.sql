CREATE TABLE superinvestor (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY, 
    portfolio_manager VARCHAR(255) NOT NULL,
    portfolio_value DECIMAL(18, 2) NOT NULL, 
    number_of_stocks INT NOT NULL, 
    manager_link VARCHAR(255) NOT NULL, 
    manager_base64 TEXT,
    updated_at TIMESTAMPTZ NOT NULL 
);
