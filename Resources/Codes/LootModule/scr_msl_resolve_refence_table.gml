function scr_msl_resolve_refence_table(argument0)
{
    var objectName = object_get_name(argument0.object_index);
    var refFile = file_text_open_read("reference_table.json"); 
    var refJson = file_text_read_string(refFile);
    var refData = json_parse(refJson);

    var min_lvl = scr_globaltile_dungeon_get("mob_lvl_min");
    var max_lvl = scr_globaltile_dungeon_get("mob_lvl_max");
    var tier = floor(((max_lvl + min_lvl) / 2));
    scr_msl_log("current tier: " + string(tier));

    if (!variable_struct_exists(refData, objectName))
    {
        scr_msl_log("cant find object " + objectName);
        file_text_close(refFile);
        return -4;
    }
    var refStruct = variable_struct_get(refData, objectName);
    var referenceLootTableIndex = -1;

    if (!variable_struct_exists(refStruct, "DefaultTable"))
    {
        scr_msl_log("cant find DefaultTable");
        file_text_close(refFile);
        return -4;
    }
    var defaultTable = variable_struct_get(refStruct, "DefaultTable");
    
    if (!variable_struct_exists(refStruct, "Ids"))
    {
        scr_msl_log("cant find Ids");
        file_text_close(refFile);
        return -4;
    }
    var idsStruct = variable_struct_get(refStruct, "Ids");

    var _ids = variable_struct_get_names(idsStruct);
    for (var i = 0; i < array_length(_ids); i++;)
    {
        if (real(_ids[i]) == argument0.id)
        {
            var referenceLootTable = variable_struct_get(idsStruct, _ids[i]);
            scr_msl_log("ref with id: " + referenceLootTable);
            file_text_close(refFile);
            return referenceLootTable;
        }
    }

    if (!variable_struct_exists(refStruct, "Tiers"))
    {
        scr_msl_log("cant find Tiers");
        file_text_close(refFile);
        return -4;
    }
    var tiersStruct = variable_struct_get(refStruct, "Tiers");
    var tiers = variable_struct_get_names(tiersStruct);
    var indexTier = -1;

    for (var i = 0; i < array_length(tiers); i++;)
    {
        if (tier < real(tiers[i]))
        {
            indexTier = i - 1;
            break;
        }
        else
        {
            indexTier = i;
        }
    }

    if (indexTier == -1)
    {
        var referenceLootTable = defaultTable;
        scr_msl_log("ref with default: " + referenceLootTable);
    }
    else
    {
        var referenceLootTable = variable_struct_get(tiersStruct, tiers[indexTier]);
        scr_msl_log("ref with tier: " + referenceLootTable);
    }
    
    file_text_close(refFile);
    return referenceLootTable;
}