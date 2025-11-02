import { Button } from "./components/ui/button";
import { useState } from "react";
function App() {
  const [count, setCount] = useState(0);
  return (
    <>
      <h1>This is dev</h1>
      <Button onClick={() => setCount(count + 1)}>Click me</Button>
      <p>Count: {count}</p>
    </>
  );
}

export default App;
