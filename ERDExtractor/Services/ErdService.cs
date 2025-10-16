using System.Text;
using System.Xml;
using ERDExtractor.Models;

namespace ERDExtractor.Services;

public class ErdService
{
    public string GenerateDrawIoXml(DatabaseSchema schema)
    {
        var sb = new StringBuilder();
        using var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true });
        
        writer.WriteStartElement("mxfile");
        writer.WriteAttributeString("host", "Electron");
        writer.WriteAttributeString("modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        writer.WriteAttributeString("agent", "ERD Extractor");
        writer.WriteAttributeString("version", "1.0");
        writer.WriteAttributeString("type", "device");
        
        writer.WriteStartElement("diagram");
        writer.WriteAttributeString("name", "ERD");
        writer.WriteAttributeString("id", Guid.NewGuid().ToString());
        
        writer.WriteStartElement("mxGraphModel");
        writer.WriteAttributeString("dx", "1200");
        writer.WriteAttributeString("dy", "800");
        writer.WriteAttributeString("grid", "1");
        writer.WriteAttributeString("gridSize", "10");
        writer.WriteAttributeString("guides", "1");
        writer.WriteAttributeString("tooltips", "1");
        writer.WriteAttributeString("connect", "1");
        writer.WriteAttributeString("arrows", "1");
        writer.WriteAttributeString("fold", "1");
        writer.WriteAttributeString("page", "1");
        writer.WriteAttributeString("pageScale", "1");
        writer.WriteAttributeString("pageWidth", "827");
        writer.WriteAttributeString("pageHeight", "1169");
        writer.WriteAttributeString("math", "0");
        writer.WriteAttributeString("shadow", "0");
        
        writer.WriteStartElement("root");
        
        // Root cells
        writer.WriteStartElement("mxCell");
        writer.WriteAttributeString("id", "0");
        writer.WriteEndElement();
        
        writer.WriteStartElement("mxCell");
        writer.WriteAttributeString("id", "1");
        writer.WriteAttributeString("parent", "0");
        writer.WriteEndElement();
        
        // Generate tables
        int x = 50;
        int y = 50;
        int cellId = 2;
        var tableIds = new Dictionary<string, int>();
        
        foreach (var table in schema.Tables)
        {
            var tableId = cellId++;
            tableIds[$"{table.Schema}.{table.TableName}"] = tableId;
            
            // Table container
            writer.WriteStartElement("mxCell");
            writer.WriteAttributeString("id", tableId.ToString());
            writer.WriteAttributeString("value", $"{table.Schema}.{table.TableName}");
            writer.WriteAttributeString("style", "swimlane;fontStyle=1;childLayout=stackLayout;horizontal=1;startSize=30;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;");
            writer.WriteAttributeString("vertex", "1");
            writer.WriteAttributeString("parent", "1");
            
            writer.WriteStartElement("mxGeometry");
            writer.WriteAttributeString("x", x.ToString());
            writer.WriteAttributeString("y", y.ToString());
            writer.WriteAttributeString("width", "200");
            writer.WriteAttributeString("height", (30 + table.Columns.Count * 26).ToString());
            writer.WriteAttributeString("as", "geometry");
            writer.WriteEndElement(); // mxGeometry
            
            writer.WriteEndElement(); // mxCell
            
            // Add columns
            foreach (var column in table.Columns)
            {
                var columnId = cellId++;
                var pkIndicator = column.IsPrimaryKey ? "🔑 " : "";
                var nullIndicator = column.IsNullable ? "" : " NOT NULL";
                var columnLabel = $"{pkIndicator}{column.ColumnName}: {column.DataType}{nullIndicator}";
                
                writer.WriteStartElement("mxCell");
                writer.WriteAttributeString("id", columnId.ToString());
                writer.WriteAttributeString("value", columnLabel);
                writer.WriteAttributeString("style", "text;strokeColor=none;fillColor=none;align=left;verticalAlign=middle;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;");
                writer.WriteAttributeString("vertex", "1");
                writer.WriteAttributeString("parent", tableId.ToString());
                
                writer.WriteStartElement("mxGeometry");
                writer.WriteAttributeString("y", (30 + table.Columns.IndexOf(column) * 26).ToString());
                writer.WriteAttributeString("width", "200");
                writer.WriteAttributeString("height", "26");
                writer.WriteAttributeString("as", "geometry");
                writer.WriteEndElement(); // mxGeometry
                
                writer.WriteEndElement(); // mxCell
            }
            
            // Update position for next table
            x += 250;
            if (x > 800)
            {
                x = 50;
                y += 300;
            }
        }
        
        // Generate foreign key relationships
        foreach (var table in schema.Tables)
        {
            var sourceTableKey = $"{table.Schema}.{table.TableName}";
            if (!tableIds.TryGetValue(sourceTableKey, out var sourceTableId))
                continue;
                
            foreach (var fk in table.ForeignKeys)
            {
                var targetTableKey = $"{fk.ReferencedSchema}.{fk.ReferencedTable}";
                if (!tableIds.TryGetValue(targetTableKey, out var targetTableId))
                    continue;
                    
                var edgeId = cellId++;
                
                writer.WriteStartElement("mxCell");
                writer.WriteAttributeString("id", edgeId.ToString());
                writer.WriteAttributeString("value", fk.ForeignKeyName);
                writer.WriteAttributeString("style", "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=ERmany;startArrow=ERone;");
                writer.WriteAttributeString("edge", "1");
                writer.WriteAttributeString("parent", "1");
                writer.WriteAttributeString("source", sourceTableId.ToString());
                writer.WriteAttributeString("target", targetTableId.ToString());
                
                writer.WriteStartElement("mxGeometry");
                writer.WriteAttributeString("relative", "1");
                writer.WriteAttributeString("as", "geometry");
                writer.WriteEndElement(); // mxGeometry
                
                writer.WriteEndElement(); // mxCell
            }
        }
        
        writer.WriteEndElement(); // root
        writer.WriteEndElement(); // mxGraphModel
        writer.WriteEndElement(); // diagram
        writer.WriteEndElement(); // mxfile
        
        writer.Flush();
        return sb.ToString();
    }
}
