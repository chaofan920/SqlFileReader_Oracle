select
    -- a
    CONCAT(a
	-- ||
	,
    -- 456
    -- 456
    456) CONCAT(table.b
	-- ||
	,
    lpad(hello,1,'2')
	-- ||
	,
    lpad(hello,2,'3')
	-- ||
	,
    lpad(hello,3,'4')
	-- ||
	,
    -- lpad(hello,4,'5')
    lpad(hello,4,'5'))
from
    table7
where
    -- lpad(hello,1,'2')
    CONCAT(lpad(hello,1,'2')
	-- ||
	,
    lpad(hello,2,'3')
	-- ||
	,
    lpad(hello,3,'4')
	-- ||
	,
    -- lpad(hello,4,'5')
    lpad(hello,4,'5'))