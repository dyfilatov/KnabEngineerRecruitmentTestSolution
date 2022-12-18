# Technical questions

## 1. How long did you spend on the coding assignment?

15 - read and understand task, estimate time  
1h - init solution  
1h - compare APIs  
3h - infrastructure implementation with caching  
3h - test coverage and debug  
Total coding assignment - 8.25h

### What would you add to your solution if you had more time? If you didn't spend much time on the coding assignment then use this as an opportunity to explain what you would add.

The most important thing I would do is to decouple reading and writing logic by adding recurring job with retries to save quotes to the database. So this lets us remove caching and in-memory semaphore.
Might be useful features from the top of my mind:
1. More validation with logging for receiving quotes, so we will know if the protocol change.
2. Authorization.
3. Fallback from one source to another (if we can't get data from one source, then try to get data from another).
4. Rate limiter.
5. Monitoring, alerts, and metrics.
6. User interface?
7. Circuit breaker (do not need in case of decoupling reading and writing).
8. Add more details about what goes wrong when an exception occurs.
9. More input parameters to quotes Endpoint, e.g. ToCurrencies array (if it's aligned with business cases).
10. Save all currencies instead of saving only currently required.
## 2. What was the most useful feature that was added to the latest version of your language of choice?
Raw string literals in C# 11.
Problem: there are a lot of excess quotes that prevent convenient working (e.g. copy-paste) with JSON.

    var jsonExample =  @"
    {
      ""Logging"": {
        ""LogLevel"": {
          ""Default"": ""Information"",
          ""Microsoft.AspNetCore"": ""Warning""
        }
      },
      ""AllowedHosts"": ""*""
    }
    ";
    var obj = JsonSerializer.Deserialize<object>(jsonExample);


Feature:

    var jsonExample = """ 
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    """;
    var obj = JsonSerializer.Deserialize<object>(jsonExample);
You can wrap a string with three double quotes and then work with the original JSON.

## 3. How would you track down a performance issue in production? Have you ever had to do this?
Yes, I have solved several performance issues in production. In most cases, it's impossible to reproduce the problem locally or in the staging environment, so I'll write about the most efficient approach without reproduction.
1.  First, I would collect as much information as possible to localize the problem:
    a. Identify as many problematic cases as possible.
    b. Look at the code, metrics, logs, and entities in databases.
    d. Run performance profilers like dottrace.
    c. Analyze information deeply. Answer the questions:
       When did the problem occur?
       Is it related to a service deployment or infrastructure works?
       What is the scope of the problem?
2.  At this stage, we might already localize the problem, and we can fix it. Otherwise:
    a.  Build hypotheses.
    b.  Add more logs and metrics.
3. Test hypotheses. This could involve optimizing code, increasing resources, changing configurations, or adding indices.

## 4. What was the latest technical book you have read or tech conference you have been to? What did you learn?
After 2019 most conferences are held remotely, so I often participate in popular dotnet conferences called DOTNEXT.
You can learn more about it here https://dotnext.ru/en/ .
When I was participating DotNext 2022 Spring I learned from cases how to implement exactly once reliable and fast delivery between two services.
Also I have read "No Rules Rules: Netflix and the Culture of Reinvention" (not sure about technical aspect of this book).
The most technical book I read is "CLR via C#" by Jeffrey Richter. He spoke at the 2018 DOTNEXT conference in Moscow and signed me this book.
You can listen his speech here https://www.youtube.com/watch?v=xGSabgBo-S8&ab_channel=DotNext


## 4. What do you think about this technical assessment?
I've made a similar coding task in CloudPayments, the difference is that we saved card number ranges instead of quotes.
The technical assessment is very good. I enjoyed it. 
I tried to follow the KISS principle, but I probably failed because the coding task describes only one simple story (too easy) and I tried to predict different cases.
In the case of real work, I would ask questions about more cases and the real user problem.

## 4. Please, describe yourself using JSON

    {
        "species": {
            "name": "human",
            "classification": {
                "kingdom": "Animalia",
                "phylum": "Chordata",
                "class": "Mammalia",
                "order": "Primates",
                "family": "Hominidae",
                "genus": "Homo",
                "species": "H. sapiens"
            },
            "distinguishing_features": "opposable thumbs, high level of intelligence and tool use"
        },
        "gender": "male",
    	"pronunciation" : "He/Him",
        "years_of_experience": 6,
        "languages": [
            "English",
            "Russian"
        ],
        "family": {
            "wife": {
                "name": "Aleksandra"
            },
            "pets": [
                {
                    "species": "cat",
                    "name": "Zlata"
                }
            ]
        },
        "occupation": "senior software engineer",
        "programming_languages": [
            "C#",
            "JavaScript",
            "SQL"
        ],
        "work_experience": [
            {
                "company": "CloudPayments",
                "position": "Software Development Team Lead",
                "duration": "2 years"
            },
            {
                "company": "CloudPayments",
                "position": "Lead Web Developer",
                "duration": "2 years"
            },
            {
                "company": "Tinkoff",
                "position": "Full stack Developer",
                "duration": "2 and a half years"
            }
        ],
        "education": {
            "degree": "bachelor",
            "field_of_study": "Software Engineering",
    		"university" : "Higher School of Economics"
        },
    	"hobbies": [
            "video games (especially FPS)",
            "gym 2-3 times per week"
        ],
        "job_search": {
            "status": "seeking",
            "preferred_industry": "fintech",
            "willing_to_relocate": true
        }
    }




