cumulative_time += delta_time / 1000000;
if (cumulative_time > end_time)
{
    if (func != noone)
    {
        script_execute(func)
    }
    instance_destroy();
}