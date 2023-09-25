package main

import (
	"fmt"
	"os/exec"
)

func main() {
	provider := "xdg-open"
	fmt.Println(exec.LookPath(provider))

	cmd := exec.Command(provider, "https://www.google.com")
	cmd.Run()
}
