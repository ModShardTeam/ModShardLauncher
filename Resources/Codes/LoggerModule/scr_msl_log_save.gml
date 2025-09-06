function scr_msl_log_save()
{
    if (global._msl_log.save_in_progress) return;

    global._msl_log.save_in_progress = true;
    var nfile_name = global._msl_log.name + "_" + string(global._msl_log.nfile) + ".txt";
    buffer_save_async(global._msl_log.buf, nfile_name, 0, global._msl_log.cur_size);

    global._msl_log.save_in_progress = false;
    instance_destroy(global._msl_log.timer);
}