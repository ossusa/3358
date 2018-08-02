require('./vendor/bootstrap')();

// config
require('./config/routes')();
require('./config/xhr')();

// factories
require('./factories/api')();

// services
require('./services/util')();
require('./services/searchBlox')();

// controllers
require('./controllers/main')();
require('./controllers/results')();
