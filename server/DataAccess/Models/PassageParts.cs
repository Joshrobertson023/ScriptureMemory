using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models;
public class PassageParts
{
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public List<string> VerseParts { get; set; } = new();
    public string Text { get; set; } = string.Empty;
     public PassageParts(string book, int chapter, List<string> verseParts, string text)
    {
        Book = book;
        Chapter = chapter;
        VerseParts = verseParts;
        Text = text;
    }
     public PassageParts() { }
}
