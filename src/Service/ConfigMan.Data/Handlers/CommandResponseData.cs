using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JasperFx.CodeGeneration.Frames;

namespace ConfigMan.Data.Handlers;

public class CommandResponse
{
    public bool WasSuccessful => !Errors.Any();
    public List<Errors> Errors { get; set; } = new();
    public long NewVersion { get; set; }


    public static CommandResponse FromError(Errors error)
    {
        return new CommandResponse { Errors = new List<Errors>() { error } };
    }

    public static CommandResponse FromSuccess(long newVersion)
    {
        return new CommandResponse { NewVersion = newVersion };
    }
}

public class CommandResponseData<T>
{
    public bool WasSuccessful => !Errors.Any();
    public List<Errors> Errors { get; set; } = new();
    public long NewVersion { get; set; }
    public T Data { get; set; } = default!;

    public static CommandResponseData<T> FromError(Errors error)
    {
        return new CommandResponseData<T> { Errors = new List<Errors>() { error } };
    }

    public static CommandResponseData<T> FromSuccess(T result, long newVersion)
    {
        return new CommandResponseData<T> { Data = result, NewVersion = newVersion };
    }
}