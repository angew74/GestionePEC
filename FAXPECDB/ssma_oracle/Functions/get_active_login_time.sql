
create function [ssma_oracle].[get_active_login_time]()
returns datetime
as begin
declare @current_login_time datetime,@login_time datetime
select @login_time=login_time from sys.dm_exec_sessions where session_id=@@spid
select @current_login_time=cast(v_value as datetime) from ssma_oracle.db_storage with (READCOMMITTEDLOCK)
where login_time=@login_time and spid=@@spid and name='$login_time$'
and exists (select * from ssma_oracle.db_storage with (READCOMMITTEDLOCK) where 
login_time=@login_time and spid=@@spid and name='$spid$')
return isnull(@current_login_time,@login_time)
end

