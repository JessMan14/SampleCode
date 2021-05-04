CREATE DEFINER=`admin`@`%` PROCEDURE `sp_get_logs`(
    in InSeverity varchar(64),
    in InStartDate datetime,
    in InEndDate datetime,
    in InTitle varchar(64),
    in InMethod varchar(64),
    in InContent varchar(4096)
)
    DETERMINISTIC
    SQL SECURITY INVOKER
    COMMENT 'Input parameters:\n   Severity varchar(64)\n   StartDate datetime\n   EndDate datetime\n   Title varchar(64)\n   Method varchar(64)\n   Content varchar(4096)'
BEGIN
    CREATE TEMPORARY TABLE IF NOT EXISTS tempTable(
    id INT NOT NULL,
    severity VARCHAR(64) NOT NULL, 
    createDate datetime NOT NULL, 
    title VARCHAR(64) NOT NULL,
    method VARCHAR(64) NOT NULL,
    content VARCHAR(4096) NOT NULL);
    
    CREATE TEMPORARY TABLE IF NOT EXISTS masterTempTable(
    id INT NOT NULL,
    severity VARCHAR(64) NOT NULL, 
    createDate datetime NOT NULL, 
    title VARCHAR(64) NOT NULL,
    method VARCHAR(64) NOT NULL,
    content VARCHAR(4096) NOT NULL);

    set @startDate = InStartDate;
    set @endDate = InEndDate;
    set @_severity = InSeverity;
    set @_title = InTitle;
    set @_method = InMethod;
    set @_content = InContent;
    
    if (@startDate is not null and @endDate is null) then
        insert into masterTempTable select * from operations_log where createDate >= @startDate;
    elseif (@startDate is null and @endDate is not null) then
        insert into masterTempTable select * from operations_log where createDate <= @endDate;
    elseif (@startDate is not null and @endDate is not null) then
        insert into masterTempTable select * from operations_log where createDate between @startDate and @endDate;
    else
        insert into masterTempTable select * from operations_log;
    end if;
    
    if (@_severity is not null) then
        insert into tempTable select * from masterTempTable WHERE locate(@_severity,severity)>0; 
        TRUNCATE TABLE masterTempTable;
        insert into masterTempTable select * from tempTable;
        TRUNCATE TABLE tempTable;
    end if;
    if (@_title is not null) then
        insert into tempTable select * from masterTempTable WHERE locate(@_title,title)>0; 
        TRUNCATE TABLE masterTempTable;
        insert into masterTempTable select * from tempTable;
        TRUNCATE TABLE tempTable;
    end if;
    if (@_method is not null) then
        insert into tempTable select * from masterTempTable WHERE locate(@_method,method)>0; 
        TRUNCATE TABLE masterTempTable;
        insert into masterTempTable select * from tempTable;
        TRUNCATE TABLE tempTable;
    end if;
    if (@_content is not null) then
        insert into tempTable select * from masterTempTable WHERE locate(@_content,content)>0; 
        TRUNCATE TABLE masterTempTable;
        insert into masterTempTable select * from tempTable;
        TRUNCATE TABLE tempTable;
    end if;


    select * from masterTempTable;
    DROP TABLE tempTable;
    DROP TABLE masterTempTable;

END