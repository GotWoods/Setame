﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Setame.Data.Models;

public class VariableGroup
{
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "jsonb")]
    public List<Setting> Settings { get; set; } = new();
}