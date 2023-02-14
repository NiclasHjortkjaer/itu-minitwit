# -*- mode: ruby -*-
# vi: set ft=ruby :

$version = Time.now.to_i

Vagrant.configure("2") do |config|
  config.vm.box = 'digital_ocean'
  config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
  config.ssh.private_key_path = '~/.ssh/id_rsa'
  config.vm.synced_folder ".", "/vagrant", type: "rsync"

  config.vm.define "minitwitserver#{$version}", primary: false do |server|

    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-18-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
      provider.privatenetworking = true
    end

    server.vm.hostname = "minitwitserver#{$version}"

    server.vm.provision "shell", inline: <<-SHELL
      echo "================================================================="
      echo "=                       INSTALLING DOCKER                       ="
      echo "================================================================="
      sudo apt-get update
      sudo apt-get install -y apt-transport-https ca-certificates curl gnupg-agent software-properties-common
      curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
      sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
      sudo apt-get update
      sudo apt-get install -y docker-ce docker-ce-cli containerd.io

      echo "================================================================="
      echo "=                       COPYING FILES                           ="
      echo "================================================================="
      cp -r /vagrant/* $HOME

      echo "================================================================="
      echo "=                       BUILDING IMAGE                          ="
      echo "================================================================="
      docker build -t minitwit -f MiniTwit/Dockerfile .

      echo "================================================================="
      echo "=                       RUNNNING IMAGE                          ="
      echo "================================================================="
      docker run -p 80:7112 -e ASPNETCORE_URLS=http://+:7112/ --name mini minitwit &

      echo "================================================================="
      echo "=                            DONE                               ="
      echo "================================================================="
      echo "Navigate in your browser to:"
      THIS_IP=`hostname -I | cut -d" " -f1`
      echo "http://${THIS_IP}"
    SHELL
  end
end