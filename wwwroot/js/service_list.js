function printServiceForm(modelHtml)
{
var Data = '<html><head><link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css"></head><body>'  + modelHtml + '</body><script>window.print();window.close();</script></html>';

      var  newwindow = window.open();
      var newdocument = newwindow.document;
        newdocument.write(Data);
        newdocument.close();
}