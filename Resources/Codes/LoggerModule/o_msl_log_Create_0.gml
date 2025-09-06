size = 1000000
buf = buffer_create(size, buffer_wrap, 1);
cur_size = 0
nfile = 0
save_in_progress = false

var curr_time = date_current_datetime();
var format_time = string_format(date_get_year(curr_time), 2, 0) + string_format(date_get_month(curr_time), 2, 0) + string_format(date_get_day(curr_time), 2, 0) + "_" + string_format(date_get_hour(curr_time), 2, 0) + string_format(date_get_minute(curr_time), 2, 0);
name = "Logs/msl_log_" + string_replace_all(format_time, " ", "0");

timer = -4