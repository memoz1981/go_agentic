using System.Text.RegularExpressions;
using static UdemyAICourseNotes.Samples._20._23_AI_As_Data_Filter;

namespace UdemyAICourseNotes.Services.Filters;

internal class BookFilterService
{
    private List<Book> _allBooks;

    public BookFilterService(List<Book> allBooks)
    {
        _allBooks = allBooks;
    }

    public IReadOnlyList<Book> GetAll() => _allBooks.AsReadOnly();

    /// <summary>
    /// Copied from https://github.com/rwjdk/MicrosoftAgentFrameworkSamples/blob/d35f7391e5af1addf9a94014d1d948e663b2012a/src/AIAsDataFilter/Program.cs#L81
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IReadOnlyList<Book> Filter(BookFilter[] filters)
    {
        var filteredBooks = _allBooks.Select(b => b);
        foreach (BookFilter filter in filters)
        {
            switch (filter.Operation)
            {
                case Operation.Equals:
                    filteredBooks = filter.Field switch
                    {
                        BookField.Title => filteredBooks.Where(x => x.Title.Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease.ToString().Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Author => filteredBooks.Where(x => x.Author.Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Genre => filteredBooks.Where(x => x.Genre.ToString().Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Synopsis => filteredBooks.Where(x => x.Synopsis.Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.NotEquals:
                    filteredBooks = filter.Field switch
                    {
                        BookField.Title => filteredBooks.Where(x => !x.Title.Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.YearOfRelease => filteredBooks.Where(x => !x.YearOfRelease.ToString().Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Author => filteredBooks.Where(x => !x.Author.Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Genre => filteredBooks.Where(x => !x.Genre.ToString().Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Synopsis => filteredBooks.Where(x => !x.Synopsis.Equals(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.StartsWith:
                    filteredBooks = filter.Field switch
                    {
                        BookField.Title => filteredBooks.Where(x => x.Title.StartsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease.ToString().StartsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Author => filteredBooks.Where(x => x.Author.StartsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Genre => filteredBooks.Where(x => x.Genre.ToString().StartsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Synopsis => filteredBooks.Where(x => x.Synopsis.StartsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.EndsWith:
                    filteredBooks = filter.Field switch
                    {
                        BookField.Title => filteredBooks.Where(x => x.Title.EndsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease.ToString().EndsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Author => filteredBooks.Where(x => x.Author.EndsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Genre => filteredBooks.Where(x => x.Genre.ToString().EndsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Synopsis => filteredBooks.Where(x => x.Synopsis.EndsWith(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.Contains:
                    filteredBooks = filter.Field switch
                    {
                        BookField.Title => filteredBooks.Where(x => x.Title.Contains(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease.ToString().Contains(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Author => filteredBooks.Where(x => x.Author.Contains(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Genre => filteredBooks.Where(x => x.Genre.ToString().Contains(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        BookField.Synopsis => filteredBooks.Where(x => x.Synopsis.Contains(filter.Value, StringComparison.CurrentCultureIgnoreCase)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.GreaterThan:
                    filteredBooks = filter.Field switch
                    {
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease > Convert.ToInt32(filter.Value)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.GreaterThanOrEqual:
                    filteredBooks = filter.Field switch
                    {
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease >= Convert.ToInt32(filter.Value)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.LessThan:
                    filteredBooks = filter.Field switch
                    {
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease < Convert.ToInt32(filter.Value)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.LessThanOrEqual:
                    filteredBooks = filter.Field switch
                    {
                        BookField.YearOfRelease => filteredBooks.Where(x => x.YearOfRelease <= Convert.ToInt32(filter.Value)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case Operation.Regex:
                    filteredBooks = filter.Field switch
                    {
                        BookField.Title => filteredBooks.Where(x => Regex.IsMatch(x.Title, filter.Value, RegexOptions.IgnoreCase)),
                        BookField.Author => filteredBooks.Where(x => Regex.IsMatch(x.Author, filter.Value, RegexOptions.IgnoreCase)),
                        BookField.Genre => filteredBooks.Where(x => Regex.IsMatch(x.Genre.ToString(), filter.Value, RegexOptions.IgnoreCase)),
                        BookField.Synopsis => filteredBooks.Where(x => Regex.IsMatch(x.Synopsis, filter.Value, RegexOptions.IgnoreCase)),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return filteredBooks.ToList();
    }
}
