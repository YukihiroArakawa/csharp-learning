string? name = null;
Console.WriteLine(name?.Length ?? 0);

var classFirst = new PersonClass("Ada");
var classSecond = new PersonClass("Ada");

var recordFirst = new PersonRecord("Ada");
var recordSecond = new PersonRecord("Ada");

Console.WriteLine(classFirst == classSecond);
Console.WriteLine(recordFirst == recordSecond);
Console.WriteLine(recordFirst.Equals(recordSecond));

var structFirst = new PersonStruct("Ada");
var structSecond = structFirst;
structSecond.Name = "Grace";

var recordStructFirst = new PersonRecordStruct("Ada");
var recordStructSecond = recordStructFirst;
recordStructSecond.Name = "Grace";

Console.WriteLine(structFirst.Name);
Console.WriteLine(structSecond.Name);
Console.WriteLine(recordStructFirst.Name);
Console.WriteLine(recordStructSecond.Name);

Console.WriteLine(new PersonStruct("Ada").Equals(new PersonStruct("Ada")));
Console.WriteLine(new PersonRecordStruct("Ada").Equals(new PersonRecordStruct("Ada")));
