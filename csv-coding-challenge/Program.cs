using csv_coding_challenge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csv_coding_challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            // create empty lists for incoming data stream
            List<OurData> ourData = new List<OurData>();
            List<TheirData> theirData = new List<TheirData>();

            // set the base file path
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;

            // parse ourdata.csv
            using (var reader = new StreamReader(filePath + "/Files/OurData.csv"))
            {
                // read a line to pass through the header row
                reader.ReadLine();

                // read through the remainer of lines
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    // skip bad tracking number values that aren't going to match
                    if (string.IsNullOrEmpty(values[2]) || values[2] == "null")
                    {
                        continue;
                    }
                    OurData data = new OurData()
                    {
                        Id = int.Parse(values[0]),
                        Name = values[1],
                        TrackingID = values[2]
                    };
                    ourData.Add(data);
                }
            }
            // parse theirdata.csv
            using (var reader = new StreamReader(filePath + "/Files/TheirData.csv"))
            {

                // read a line to pass through the header row
                reader.ReadLine();

                // read through the remainer of lines
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    TheirData data = new TheirData()
                    {
                        Creative = values[0],
                        Clicks = int.Parse(values[1]),
                        Impressions = int.Parse(values[2]),
                        DateStamp = DateTime.Parse(values[3])
                    };
                    theirData.Add(data);
                }
            }

            // order theirdata
            theirData = theirData.OrderBy(o => o.DateStamp).OrderBy(o => o.Creative).ToList();

            // create empty lists for resulting data stream
            List<CombinedData> combinedData = new List<CombinedData>();

            // join together records with duplicate
            for (int i = 0; i < theirData.Count; )
            {
                List<TheirData> data = theirData.Where(x => x.Creative == theirData[i].Creative).ToList();

                combinedData.Add(new CombinedData()
                {
                    Creative = theirData[i].Creative,
                    Clicks = data.Sum(x => x.Clicks),
                    Impressions = data.Sum(x => x.Impressions),
                    DateStamp = theirData[i].DateStamp
                });
                i += data.Count;
            }

            for (int i = 0; i < combinedData.Count; i++)
            {
                OurData data = ourData.Where(x => combinedData[i].Creative.Contains(x.TrackingID)).FirstOrDefault();
                if (data == null)
                {
                    continue;
                }
                combinedData[i].Id = data.Id;
                combinedData[i].Name = data.Name;
                combinedData[i].TrackingID = data.TrackingID;
            }

            // path where generated csv will be output
            string outputFilePath = filePath + "/Files/output_" + DateTime.Now.Ticks + ".csv";

            Type itemType = typeof(CombinedData);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // open stream writer to populate csv
            using (var writer = new StreamWriter(outputFilePath))
            {
                // write in the headers
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in combinedData)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }
    }
}
