using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Web;
using System.Data;
using PRoCon.Core;

public class BattlelogClient
{

    private HttpWebRequest req = null;
    public String personaID = "";

    WebClient client = null;

    private String fetchWebPage(ref String html_data, String url)
    {
        try
        {
            if (client == null)
                client = new WebClient();

            html_data = client.DownloadString(url);
            return html_data;

        }
        catch (WebException e)
        {
            if (e.Status.Equals(WebExceptionStatus.Timeout))
                throw new Exception("HTTP request timed-out");
            else
                throw;
        }
    }


    public Hashtable getStats(String player, int kit_index = 0)
    {
        try
        {
            /* First fetch the player's main page to get the persona id */
            String result = "";
            fetchWebPage(ref result, "http://battlelog.battlefield.com/bfh/user/" + player);

            string decoded = HttpUtility.HtmlDecode(result);
            String tag = extractClanTag(decoded, player);

            /* Extract the persona id */
            MatchCollection pid = Regex.Matches(decoded, @"bfh/agent/" + player + @"/stats/(\d+)(/\w*)?/", RegexOptions.Singleline);

            String personaId = "";

            foreach (Match m in pid)
            {
                if (m.Success && m.Groups[2].Value.Trim() == "/pc")
                {
                    personaId = m.Groups[1].Value.Trim();
                }
            }
            personaID = personaId;
            if (personaId == "")
                throw new Exception("could not find persona-id for ^b" + player);

            fetchWebPage(ref result, String.Format("http://battlelog.battlefield.com/bfh/bfhloadout/kit/{0}/pc/{1}", personaId, kit_index));

            Hashtable json = (Hashtable)JSON.JsonDecode(result);

            // check we got a valid response
            if (!(json.ContainsKey("type") && json.ContainsKey("message")))
                throw new Exception("JSON response does not contain \"type\" or \"message\" fields");

            String type = (String)json["type"];
            String message = (String)json["message"];

            /* verify we got a success message */
            if (!(type.StartsWith("success") && message.StartsWith("OK")))
                throw new Exception("JSON response was type=" + type + ", message=" + message);


            /* verify there is data structure */
            Hashtable data = null;
            if (!json.ContainsKey("data") || (data = (Hashtable)json["data"]) == null)
                throw new Exception("JSON response was does not contain a data field");

            data.Add("tag", tag);
            return data;
        }
        catch (Exception e)
        {
            //Handle exceptions here however you want
            Console.WriteLine(e.ToString());
        }

        return null;
    }

    public String extractClanTag(String result, String player)
    {
        /* Extract the player tag */
        Match tag = Regex.Match(result, @"\[\s*([a-zA-Z0-9]+)\s*\]\s*" + player, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (tag.Success)
            return tag.Groups[1].Value;

        return String.Empty;
    }

}