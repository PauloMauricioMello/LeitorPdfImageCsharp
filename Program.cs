using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Tesseract;
using System.Text.RegularExpressions;

var caminhoDocumento = @"D:\ADO\Laboratorio\LeitorDePdf\leitorpdf_cs\documento_para_teste\certidão negativa trabalhista.pdf";
var diretorioSaida = @"D:\ADO\Laboratorio\LeitorDePdf\leitorpdf_cs\documento_para_teste";
var tessDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";

Document pdf = new(caminhoDocumento);
PngDevice pngDevice = new(new Resolution(300));

using(var imageStream = new MemoryStream())
{
      pngDevice.Process(pdf.Pages[1], imageStream);

      imageStream.Position = 0;

      using var engine = new TesseractEngine(tessDataPath, "por", EngineMode.Default);
      using var img = Pix.LoadFromMemory(imageStream.ToArray());
      using var page = engine.Process(img);

      string extrairTexto = page.GetText();
      string informacoes = ExtrairInformacoesTexto(extrairTexto);

      string diretorioSaidaTexto = $"{diretorioSaida}/pagina_1_OCR.txt";
      File.WriteAllText(diretorioSaidaTexto, informacoes);

      Console.WriteLine($"Informações salvas em: {diretorioSaidaTexto}");
}

static string ExtrairInformacoesTexto(string texto)
{
      string nome = Regex.Match(texto, @"Nome:\s*([\s\S]*?)(?=\nCNPJ:)")?.Groups[1].Value.Trim();
      string cnpj = Regex.Match(texto, @"CNPJ:\s*([\d\.\/-]+)")?.Groups[1].Value;
      string certidao = Regex.Match(texto, @"Certidão n[ºo]*:\s*([\d/]+)")?.Groups[1].Value;
      string expedicao = Regex.Match(texto, @"Expedição:\s*(\d{2}/\d{2}/\d{4})")?.Groups[1].Value;
      string validade = Regex.Match(texto, @"Validade:\s*(\d{2}/\d{2}/\d{4})")?.Groups[1].Value;

      return $"Nome: {nome}\n" +
             $"CNPJ: {cnpj}\n" +
             $"Certidão: {certidao}\n" +
             $"Expedição: {expedicao}\n" +
             $"Validade: {validade}\n";
}
