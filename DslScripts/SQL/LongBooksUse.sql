SELECT ID, Title = lb.Title, NumberOfPages = lb.NumberOfPages, Code = lb.Code
FROM Bookstore.Bookstore.LongBooksView as lb
WHERE LOWER(lb.Title) LIKE 'c%'