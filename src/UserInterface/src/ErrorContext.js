import { createContext } from 'react';

const ErrorContext = createContext({
    errorMessage: [],
    setErrorMessage: () => {},
});

export default ErrorContext;