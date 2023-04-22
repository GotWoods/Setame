// src/components/VariableGroups.js
import React from 'react';

const VariableGroups = () => {
  return (
    <div>
      <h1>Variable Groups</h1>
      TODO: this will allow you to set re-usable sets of variables that can be attached to an application.<br /><br />

      There may be two types of groups. The first would be one that uses environments so I could create something like:<br />

      MassTransit.VirtualHost and have Dev/Stage/PreProd/Prod be different values. Then I could just attach it to the application and not need to have 4 different variable groups<br /><br />

      The second would be a stand alone group not tied to any environment. It would be attached to an application by environment.



      {/* Add your content here */}
    </div>
  );
};

export default VariableGroups;
