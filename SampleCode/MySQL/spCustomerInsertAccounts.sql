CREATE DEFINER=`dgs_admin`@`%` PROCEDURE `spCustomerInsertAccounts`()
BEGIN
	DECLARE done INT DEFAULT 0;
	DECLARE account_id varchar(25);	
	DECLARE customerName varchar(45);	
	DECLARE address varchar(45);	    
	DECLARE city varchar(45);	
	DECLARE state varchar(25);	
    DECLARE zip int DEFAULT 0;
  	
	DECLARE cur1 CURSOR FOR SELECT AccountNumber FROM DGS_Staging.Accounts as sb WHERE AccountNumber NOT IN (SELECT account_key FROM DCMetroDev.Customer );
    DECLARE cur2 CURSOR FOR SELECT CustomerName  FROM DGS_Staging.Accounts as sb WHERE AccountNumber NOT IN (SELECT account_key FROM DCMetroDev.Customer );   
    DECLARE cur3 CURSOR FOR SELECT ServiceAddress  FROM DGS_Staging.Accounts as sb WHERE AccountNumber NOT IN (SELECT account_key FROM DCMetroDev.Customer );        
    DECLARE cur4 CURSOR FOR SELECT CustomerCity  FROM DGS_Staging.Accounts as sb WHERE AccountNumber NOT IN (SELECT account_key FROM DCMetroDev.Customer );       
    DECLARE cur5 CURSOR FOR SELECT CustomerState FROM DGS_Staging.Accounts as sb WHERE AccountNumber NOT IN (SELECT account_key FROM DCMetroDev.Customer );
    DECLARE cur6 CURSOR FOR SELECT CustomerZip FROM DGS_Staging.Accounts as sb WHERE AccountNumber NOT IN (SELECT account_key FROM DCMetroDev.Customer );
    
	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    
    OPEN cur1;
    OPEN cur2;
    OPEN cur3;
    OPEN cur4;
	OPEN cur5;
    OPEN cur6;
    
	REPEAT
		FETCH cur1 INTO account_id;
        FETCH cur2 INTO customerName;
        FETCH cur3 INTO address;
        FETCH cur4 INTO city;
        FETCH cur5 INTO state;
        FETCH cur6 INTO zip;   

		IF NOT done THEN
			IF NOT EXISTS(SELECT * from DCMetroDev.Customer WHERE account_key = account_id) THEN        
				INSERT INTO DCMetroDev.Customer (account_key, `name`, address1, city, state, zip, programID)
				VALUES (account_id, customerName, address, city, state, zip, '3'); 
			END IF;
		END IF;

	  UNTIL done END REPEAT;

	  CLOSE cur1;    
	  CLOSE cur2;
	  CLOSE cur3;
	  CLOSE cur4;
	  CLOSE cur5;
	  CLOSE cur6; 

END