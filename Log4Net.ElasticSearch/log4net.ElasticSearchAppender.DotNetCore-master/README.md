log4net.ElasticSearchAppender.DotNetCore
=====================

**The project is hard forked from** [log4net.Appender.ElasticSearch](https://github.com/Elders/log4net.Appender.ElasticSearch). 

log4net.ElasticSearchAppender is a [log4net](http://logging.apache.org/log4net/) appender to log messages to the [ElasticSearch](http://www.elasticsearch.org) document database. 

### Features:
* **Targets net452;netstandard2.0;netstandard2.1**
* Easy installation and setup via [Nuget](https://www.nuget.org/packages/log4net.ElasticSearchAppender.DotNetCore/)
* Ability to analyze the log event before sending it to elasticsearch using built-in filters and custom filters similar to [logstash](http://logstash.net/docs/1.4.2/).

##### Simple configuration:
```xml
<appender name="ElasticSearchAppender" type="log4net.ElasticSearchAppender.DotNetCore.ElasticSearchAppender, log4net.ElasticSearchAppender.DotNetCore">
    <Server>localhost</Server>
    <Port>9200</Port>
</appender>
```

##### (Almost) Full configuration:
```xml
<appender name="ElasticSearchAppender" type="log4net.ElasticSearchAppender.DotNetCore.ElasticSearchAppender, log4net.ElasticSearchAppender.DotNetCore">
    <Server>localhost</Server>
    <Port>9200</Port>
    <!-- optional: in case elasticsearch is located behind a reverse proxy the URL is like http://Server:Port/Path, default = empty string -->
    <Path>/es5</Path>
    <IndexName>log_test_%{+yyyy-MM-dd}</IndexName>
    <IndexType>LogEvent</IndexType>
    <Bulksize>2000</Bulksize>
    <BulkIdleTimeout>10000</BulkIdleTimeout>
    <IndexAsync>False</IndexAsync>
    <DocumentIdSource>IdSource</DocumentIdSource> <!-- obsolete! use IndexOperationParams -->
    
    <!-- Serialize log object as json (default is true).
      -- This in case you log the object this way: `logger.Debug(obj);` and not: `logger.Debug("string");` -->
    <SerializeObjects>True</SerializeObjects> 

    <!-- optional: elasticsearch timeout for the request, default = 10000 -->
    <ElasticSearchTimeout>10000</ElasticSearchTimeout>

    <!--You can add parameters to the request to control the parameters sent to ElasticSearch.
    for example, as you can see here, you can add a routing specification to the appender.
    The Key is the key to be added to the request, and the value is the parameter's name in the log event properties.-->
    <IndexOperationParams>
      <Parameter>
        <Key>_routing</Key>
        <Value>%{RoutingSource}</Value>
      </Parameter>
      <Parameter>
        <Key>_id</Key>
        <Value>%{IdSource}</Value>
      </Parameter>
      <Parameter>
        <Key>key</Key>
        <Value>value</Value>
      </Parameter>
    </IndexOperationParams>

    <!-- for more information read about log4net.Core.FixFlags -->
    <FixedFields>Partial</FixedFields>
    
    <Template>
      <Name>templateName</Name>
      <FileName>path2template.json</FileName>
    </Template>

    <!--Only one credential type can used at once-->
    <!--Here we list all possible types-->
    <AuthenticationMethod>
      <!--For basic authentication purposes-->
      <Basic>
          <Username>Username</Username>
          <Password>Password</Password>
      </Basic>
      <!--For AWS ElasticSearch service-->
      <Aws>
          <Aws4SignerSecretKey>Secret</Aws4SignerSecretKey>
          <Aws4SignerAccessKey>AccessKey</Aws4SignerAccessKey>
          <Aws4SignerRegion>Region</Aws4SignerRegion>
      </Aws>
    </AuthenticationMethod>
    
    <!-- all filters goes in ElasticFilters tag -->
    <ElasticFilters>
      <Add>
        <Key>@type</Key>
        <Value>Special</Value>
      </Add>

      <!-- using the @type value from the previous filter -->
      <Add>
        <Key>SmartValue</Key>
        <Value>the type is %{@type}</Value>
      </Add>

      <Remove>
        <Key>@type</Key>
      </Remove>

      <!-- you can load custom filters like I do here -->
      <Filter type="log4stash.Filters.RenameKeyFilter, log4stash">
        <Key>SmartValue</Key>
        <RenameTo>SmartValue2</RenameTo>
      </Filter>
    
      <!-- converts a json object to fields in the document -->
      <Json>
        <SourceKey>JsonRaw</SourceKey>
        <FlattenJson>false</FlattenJson>
        <!-- the separator property is only relevant when setting the FlattenJson property to 'true' -->
        <Separator>_</Separator> 
      </Json>

      <!-- converts an xml object to fields in the document -->
      <Xml>
        <SourceKey>XmlRaw</SourceKey>
        <FlattenXml>false</FlattenXml>
      </Xml>
      
      <!-- kv and grok filters similar to logstash's filters -->
      <Kv>
        <SourceKey>Message</SourceKey>
        <ValueSplit>:=</ValueSplit>
        <FieldSplit> ,</FieldSplit>
      </kv>

      <Grok>
        <SourceKey>Message</SourceKey>
        <Pattern>the message is %{WORD:Message} and guid %{UUID:the_guid}</Pattern>
        <Overwrite>true</Overwrite>
      </Grok>

      <!-- Convert string like: "1,2, 45 9" into array of numbers [1,2,45,9] -->
      <ConvertToArray>
        <SourceKey>someIds</SourceKey>
        <!-- The separators (space and comma) -->
        <Seperators>, </Seperators> 
      </ConvertToArray>

      <Convert>
        <!-- convert given key to string -->
        <ToString>shouldBeString</ToString>

        <!-- same as ConvertToArray. Just for convenience -->
        <ToArray>
           <SourceKey>anotherIds</SourceKey>
        </ToArray>
      </Convert>
    </ElasticFilters>
</appender>
```
