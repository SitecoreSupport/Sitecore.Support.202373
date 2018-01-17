using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Form.Core.Configuration;
using Sitecore.Publishing;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Support.Form.Core
{
  public class PublishItemExtension
  {
    private const string GuidPattern = "(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}";

    public void PublishFormChildItems(object sender, EventArgs args)
    {
      string value = IDs.FormInterpreterID.ToString();
      string value2 = IDs.FormMvcInterpreterID.ToString();
      SitecoreEventArgs sitecoreEventArgs = args as SitecoreEventArgs;
      Publisher publisher = sitecoreEventArgs.Parameters.FirstOrDefault<object>() as Publisher;
      if (publisher.Options.PublishRelatedItems && publisher.Options.Mode == PublishMode.SingleItem)
      {
        Item rootItem = publisher.Options.RootItem;
        string value3 = rootItem.Fields["__Final renderings"].Value;
        if (value3.Contains(value) || value3.Contains(value2))
        {
          Database sourceDatabase = publisher.Options.SourceDatabase;
          MatchCollection matchCollection = Regex.Matches(value3, "(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}");
          foreach (object current in matchCollection)
          {
            Item item = sourceDatabase.GetItem(current.ToString());
            if (item != null && item.TemplateID == IDs.FormTemplateID)
            {
              this.PublishItem(item, sourceDatabase, publisher.Options.TargetDatabase, PublishMode.SingleItem);
            }
          }
        }
      }
    }

    private void PublishItem(Item item, Database sourceDB, Database targetDB, PublishMode mode)
    {
      PublishOptions options = new PublishOptions(sourceDB, targetDB, mode, item.Language, DateTime.Now)
      {
        RootItem = item,
        Deep = true
      };
      Publisher publisher = new Publisher(options);
      publisher.Publish();
    }
  }
}