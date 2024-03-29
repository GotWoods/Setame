﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Setame.Data.Models;

public class EnvironmentGroup
{
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public Dictionary<string, List<Setting>> EnvironmentSettings { get; set; } = new();
}