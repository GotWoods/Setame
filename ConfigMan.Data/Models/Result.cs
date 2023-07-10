﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Models
{
    public class Result<TValue, TError>
    {
        public bool IsError { get; }
        public bool IsSuccess => !IsError;
        private readonly TValue? _value;
        private readonly TError? _error;

        public Result(TValue value)
        {
            IsError = false;
            _value = value;
            _error = default;
        }

        public Result(TError error)
        {
            IsError = true;
            _value = default;
            _error = error;
        }
    }
}
