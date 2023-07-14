const Main = () => {
    return (
        <>
            <h2>Core Concepts</h2>

            <div>
                <p>
                Environment Sets: Environment Sets form unique combinations of environments through which an application transitions. Variables applied to an Environment Set are considered global and are inherited by all associated applications. This is an ideal place to define variables that are used universally by all applications.
                </p>
                <p>
                How To Best Use: An Environment Set should be created for each unique set of environments being used. e.g. If your development team deploys from Staging to Testing to Production, and the marketing team deploys from Preview to Production then you should have two Environment Sets (one for each team)
                </p>
                <p>
                Variables That Often Go Here: URLs used by all applications (e.g. logging server, message bus server, etc.), Company Name, Helpdesk Email, etc.
                </p>
            </div>

            <div>
                <p>
                Applications: Applications have two types of variables:
                </p>
                <p>
                Application Global Settings: These are settings unique to the application but are consistent across all its environments. These variables are directly related to the application and are passed on to each environment within the application.
                </p>
                <p>
                How to Best Use: Application Global Settings should be used for variables that are unique to the application but are consistent across all environments. e.g. If your application uses a common name for logging or accessing a resource, this is where it would best go. Also settings that are the same across all non-production environments often go here with it only being overridden for the production environment.
                </p>

                <p>
                Environment Specific Settings: These are variables defined explicitly for each individual environment within an application. They hold the highest priority and will override all other variables when a naming conflict occurs.
                </p>
                <p>
                How to Best Use: Environment Specific Settings should be used for variables that are unique to a specific environment. e.g. If your application uses a different connection string, password, URL, etc per environment .
                </p>
            </div>

            <div>
                Hierarchy Order: When a naming conflict arises, the priority order from highest to lowest is as follows: Environment Specific Settings &gt; Application Global Settings &gt; Environment Set Variables. This order dictates that the Environment Specific Settings override all others, followed by Application Global Settings, and lastly, the Environment Set Variables.
            </div>
        </>
    );
}

export default Main;