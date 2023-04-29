select
    a
	||
    table.b
	||
    lpad(hello,1,'2')
from
    table7
where
    table2.col = '123';