# Background

In my last blog post [How to implement extended audit logging in dataverse Part 1](https://blog.danijel.se/2023-01-16-how-to-implement-extended-audit-logging-in-dataverse-part-1) I showed how we can use the out of box Microsoft Purview/Office logging to send Retreieve events in Dataverse and then use Power BI to visualize this information back to users in Dataverse.

However, the entire process of setting everything up was quite cumbersome and involved a lot of moving parts. So I was in search for something simpler. The suggested solution is to use a generic plugin-assembly that you register on the events that you want to audit and this assembly will send information to an Application Insights of your choice. Then we can query the log analytics workspace with Power BI and show the audit log data. Here is an image showing what we are trying to achieve.

![Audit Log](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Extended-Audit-log.png)
_The architecture of the suggested solution._

# 1. Create an Application Insights in Azure

The first thing you should do is to go to your Azure tenant and create an Application Insights app. When you have everything set up, copy your connection string, we will use this later to connect to and send Audit data to Application Insights.

![Application Insights Connection string](https://blog.danijel.se//img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Application-Insights-Connection-string.png)
_Copy the connection string for later._

# 2. Download the Application Insights Assembly

Now we will [download the solution](https://github.com/PowerPlatformProfessor/ExtendedAuditLogPackagePart2/releases/tag/v1.0.0.0) that I built and install it to our Dataverse environment. This solution will connect to you application insights and send the data. The source code is provided, so you can extend the assembly to your needs. [The source code](https://github.com/PowerPlatformProfessor/ExtendedAuditLogPackagePart2).

# 3. Register steps

When you have installed the solution in to your Dataverse environment you can register on what events the assembly should send logs to Application Insights.

![Register steps](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Register-steps.png)
_Register the steps for the events and tables that you want to audit._

On each step make sure to paste the connection string of your application insights in the secure config part (I will improve this to use Environment Variables in the future). Notice that I have registerd the step as Post Operation and asyc in order to minimize the performance impact on the system.

![Register step details](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Register-step-details.png)
_Make sure to paste the AI connection string in the secure config part of the step registration._

# 4. Test the logging

Perform some of the events inside Dataverse (in order to capture some data). For example, in my case, I opened a contact record. This way the Retrieve event got triggered. Now, go inside Application Insights and go to logs, select customEvents and run the query. You should see your events being logged.

![Logs](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Logs.png)
_The event data from dataverse is registered in Application Insights._

# 5. Load the data to Power BI

Now we can load the data to Power BI. First go to your query in Azure and select Export -> Power BI (as an M-query).
This will download a text file with the queries. Open Power BI desktop and create a new Report. Select Get data then Other -> Blank Query.

![Power Bi Blank Query](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/PowerBI-BlankQuery.png)
_How to get the data from Application Insights in to Power BI._

Select advanced editor and paste the query from the doenloaded text file.

![Blank Query Advanced Editor](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/BlankQueryAdvancedEditor.png)
_Select advanced editor._

![Power Query Editor](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Power-Query-Editor.png)
_Paste your query here._

Above steps will load the logs. Next we need to select the fields from the customDimensions.
In order to do this we need to right click the customDimensions field and transform it to JSON. This will transform each rows column to a record.

![Transform to JSON](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Parsed-JSON.png)
_Transform the customDiemensions filed to JSON._

Split the record to its properties by pressing the button shown in below image.
![Expand JSON to fields](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Expand-json-to-fields.png)

# 6. Power BI report - End result

Now you can create the Power BI report. Below is a picture of the end result.

![Power BI report](https://blog.danijel.se/img/2023-02-08-how-to-implement-extended-audit-log-indataverse-part-2/Power-BI-report.png)
_The end result, a power bi report that can be embeded and filtered in dataverse._

The report can be embeded inside Dataverse in similar fashion as explained in [part 1 - 5. Visualize data in Power Bi and embed in Dataverse with filter](https://blog.danijel.se/2023-01-16-how-to-implement-extended-audit-logging-in-dataverse-part-1).

# Conclusion

Here you got to see yet another way to create an extended audit log. I actually prefer this solutions, since there is fewer moving parts in it. What solution do you like best? Feel free to contact me on twitter with your opinions.
