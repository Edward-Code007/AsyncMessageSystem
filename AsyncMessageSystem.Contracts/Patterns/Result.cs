namespace AsyncMessageSystem.ResultPattern;

public class Result<T> where T : class
{
    public T? Value { get; set; }
    public Error? Error { get; set; }
    public bool isAnyError = false;
    private Result(T value)
    {

        this.Value = value;
        this.Error = default;
    }
    private Result(Error error)
    {
        this.Value = default;
        this.Error = error;
        this.isAnyError = true;
    }
    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failed(string description) => new Result<T>(new Error(description));


}

public class Error(string description)
{
    public string Description { get; set; } = description;

}
