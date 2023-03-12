# -*- mode: ruby -*-
# vi: set ft=ruby :

$ip_file = "db_ip.txt"
$version = (Time.now.to_i - 1675234602) / 300

Vagrant.configure("2") do |config|
  config.vm.box = 'digital_ocean'
  config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
  config.ssh.private_key_path = '~/.ssh/do_ssh_key'
  
  config.vm.synced_folder "remote_files", "/minitwit", type: "rsync"
  config.vm.synced_folder '.', '/vagrant', type: "rsync"
  
  if !File.file?($ip_file)
    config.vm.define "minitwitdb", primary: true do |server|
      server.vm.provider :digital_ocean do |provider|
        provider.ssh_key_name = ENV["SSH_KEY_NAME"]
        provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
        provider.image = 'ubuntu-18-04-x64'
        provider.region = 'fra1'
        provider.size = 's-1vcpu-1gb'
        provider.privatenetworking = true
      end
  
      server.vm.hostname = "minitwitdb"
  
      server.trigger.after :up do |trigger|
        trigger.info =  "Writing dbserver's IP to file..."
        trigger.ruby do |env,machine|
          remote_ip = machine.instance_variable_get(:@communicator).instance_variable_get(:@connection_ssh_info)[:host]
          File.write($ip_file, remote_ip)
        end
      end
  
      server.vm.provision "shell", inline: <<-SHELL
      echo "================================================================="
      echo "=                       INSTALLING DOCKER                       ="
      echo "================================================================="
      snap install docker
      sudo apt-get update
  
      echo "================================================================="
      echo "=                DOWNLOADING AND RUNNING IMAGE                  ="
      echo "================================================================="
      docker run --name minitwitdb -e POSTGRES_PASSWORD=#{ENV["POSTGRES_PASSWORD"]} -p 5432:5432 -d postgres
  
      SHELL
    end
  end
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

    server.trigger.before :up do |trigger|
      trigger.info =  "Waiting to create server until minitwitdb's IP is available."
      trigger.ruby do |env,machine|
        ip_file = "db_ip.txt"
        while !File.file?($ip_file) do
          sleep(1)
        end
        db_ip = File.read($ip_file).strip()
        puts "Now, I have it..."
        puts db_ip
      end
    end
    
    server.vm.provision "shell", inline: 'echo "export DOCKER_USERNAME=' + "'" + ENV["DOCKER_USERNAME"] + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DOCKER_PASSWORD=' + "'" + ENV["DOCKER_PASSWORD"] + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DB_PASSWORD=' + "'" + ENV["POSTGRES_PASSWORD"] + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DB_USER=' + "'" + ENV["DB_USER"] + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DB_IP=`cat /vagrant/db_ip.txt`" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DB_HOST=' + "'" + "$DB_IP" + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DB_PORT=5432" >> ~/.bash_profile'

    server.vm.provision "shell", inline: <<-SHELL
      echo $DB_IP
  
      echo "================================================================="
      echo "=                       INSTALLING DOCKER                       ="
      echo "================================================================="
      sudo apt install -y apt-transport-https ca-certificates curl software-properties-common
      curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
      sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable"
      apt-cache policy docker-ce
      sudo apt install -y docker-ce
      sudo systemctl status docker
      sudo usermod -aG docker ${USER}
      sudo apt install -y docker-compose

      # Install make
      sudo apt-get install -y make
      
      echo -e "\nVerifying that docker works ...\n"
      docker run --rm hello-world
      docker rmi hello-world
      echo -e "\nOpening port for minitwit ...\n"
      ufw allow 80 && \
      ufw allow 22/tcp
      echo ". $HOME/.bashrc" >> $HOME/.bash_profile
      echo -e "\nConfiguring credentials as environment variables...\n"
      source $HOME/.bash_profile
      echo -e "\nSelecting Minitwit Folder as default folder when you ssh into the server...\n"
      echo "cd /minitwit" >> ~/.bash_profile
      chmod +x /minitwit/deploy.sh
      
      sh /minitwit/deploy.sh
      echo -e "\nVagrant setup done ..."
      echo -e "minitwit will be accessible at http://$(hostname -I | awk '{print $1}')"
    SHELL
  end
  config.vm.provision "shell", privileged: false, inline: <<-SHELL
    sudo apt-get update
  SHELL
end
