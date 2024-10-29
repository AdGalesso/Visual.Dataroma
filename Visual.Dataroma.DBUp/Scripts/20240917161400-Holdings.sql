CREATE TABLE holding (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY, 
    superinvestor_id UUID NOT NULL, 
    stock_code VARCHAR(10) NOT NULL,
    portfolio_percentage DECIMAL(18, 2) NOT NULL, 
    last_activity VARCHAR(100) NOT NULL,
    number_of_stocks INT NOT NULL, 
    reported_price DECIMAL(18, 2) NOT NULL, 

    CONSTRAINT fk_superinvestor FOREIGN KEY (superinvestor_id) REFERENCES superinvestors(id),
    CONSTRAINT fk_stock FOREIGN KEY (stock_code) REFERENCES stock(code)
);
